namespace :tests do
	desc "gathers the references into the build directory. selects only the ones that dont exist"
	task :test_references => :ensure_dirs do |t|
		identify t
	
		files = ["ManagedWinapi.dll",
				 "Alchemy.dll",
				 "Interop.iTunesLib.dll",
				 "Microsoft.Lync.Model.dll",
				 "Microsoft.Office.Uc.dll",
				 "nunit.framework.dll",
				 "FluentAssertions.dll",
				 "Moq.dll"].map {|filename| FOLDERS[:binaries] + filename}
		
		files << "C:\\Program Files (x86)\\Microsoft Office\\Office15\\LyncSDK\\Assemblies\\Desktop\\Microsoft.Lync.Model.dll"
		files << "C:\\Program Files (x86)\\Microsoft Office\\Office15\\LyncSDK\\Assemblies\\Desktop\\Microsoft.Office.Uc.dll"
		files << FOLDERS[:build] + "shutupify-lib.dll"
	
		copy_files_into_folder FOLDERS[:test], files
	end

	desc "places testdata into test data directory"
	task :test_data => :build_mock do |t|
		identify t
	
		test_data = FileList["Tests/TestData/**"]
		test_data.exclude /.*cs$/
		copy_files_into_folder FOLDERS[:test_data], test_data
	end
	
	desc "build mock dll for testing"
	csc :build_mock => ["win:build_lib"] do |csc|
		identify "build_mock"
	
		application_name = FOLDERS[:test_data] + "shutupify-mock.dll"
		File::delete application_name if File::exists? application_name
	
		references = FileList["#{FOLDERS[:build]}/shutupify-lib.dll"]
		references.extend(FileListEnhancement)
	
		for_compile = FileList["Tests/TestData/*.cs"] 
		puts "compiling #{for_compile.length} files."
	
		csc.use 		:net40
		csc.compile 	for_compile
		csc.references 	references
		csc.target = 	:library
		csc.output = 	application_name	
	end
	
	desc "builds the shutupify-unit test dll"
	csc :build => ["win:build_lib", :test_references, :test_data] do |csc|
		identify "build_test"
	
		output = FOLDERS[:test] + "shutupify-unit-tests.dll"
		File::delete output if File::exists? output
	
		references = FileList[FOLDERS[:binaries] + "*.dll", FOLDERS[:build] + "shutupify-lib.dll"]
		references.extend(FileListEnhancement)
	
		fail "Unit test references are not available in binary directory" unless references.has? "nunit.framework.dll",	"FluentAssertions.dll",	"Moq.dll"
	
		for_compile = FileList["Tests/Unit/*.cs"] 
		puts "compiling #{for_compile.length} files."
	
		csc.use 		:net40
		csc.compile 	for_compile
		csc.references 	references
		csc.target = 	:library
		csc.output = 	output
	end
	
	desc "run unit tests"
	nunit :run => [:build] do |nunit|
		identify "run_test"
	
		nunit.command = "C:\\Program Files (x86)\\NUnit 2.5.10\\bin\\net-2.0\\nunit-console.exe" 
	
		test_assemblies = FileList[FOLDERS[:test] + "*-tests.dll"]
		nunit.assemblies test_assemblies
	end

end