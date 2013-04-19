
namespace :win do

	debug = true

	desc "gathers the references into the build directory. selects only the ones that dont exist"
	task :references => :ensure_dirs do |t|
		identify t
	
		files = ["ManagedWinapi.dll",
				 "Alchemy.dll",
				 "Interop.iTunesLib.dll",
				 "Microsoft.Lync.Model.dll",
				 "Microsoft.Office.Uc.dll"].map {|filename| FOLDERS[:binaries] + filename}
		
		files << "C:\\Program Files (x86)\\Microsoft Office\\Office15\\LyncSDK\\Assemblies\\Desktop\\Microsoft.Lync.Model.dll"
		files << "C:\\Program Files (x86)\\Microsoft Office\\Office15\\LyncSDK\\Assemblies\\Desktop\\Microsoft.Office.Uc.dll"
	
		copy_files_into_folder FOLDERS[:build], files
	end
	
	desc "Run assembly info generator"
	assemblyinfo :assemblyinfo do |asm|
		identify "assemblyinfo"

		asm.version = asm.file_version = File.read(FOLDERS[:root] + "VERSION")
		asm.company_name = "Krizzzn"
		asm.product_name = "shutupify"
		asm.title = "shutupify, the playback tamer"
		asm.description = "An application to globally control the playback of your favorite audio player."
		asm.copyright = "2013"
		asm.copyright += " - " + Date.today.strftime("%Y") if 2013 > Date.today.year
		asm.output_file = FOLDERS[:root] + "_AssemblyInfo.cs"
	end

	desc "Remove assembly info"
	task :clean_up do |task|
		identify task

		rm FOLDERS[:root] + "_AssemblyInfo.cs"
	end
	
	desc "builds the shutupify-lib dll"
	csc :build_lib => [:references, :assemblyinfo] do |csc|
		identify "build_lib (as release: #{!debug})"

		libraryname = FOLDERS[:build] + "shutupify-lib.dll"
		File::delete libraryname if File::exists? libraryname
		
		references = FileList["#{FOLDERS[:build]}*.dll"]
		references.extend(FileListEnhancement)
		
		puts "\n"
		puts "Lync Probe was not added because reference to Microsoft.Lync.Model.dll, Microsoft.Office.Uc.dll is missing." unless references.has? "Microsoft.Lync.Model.dll", "Microsoft.Office.Uc.dll"
		puts "Itunes Controller was not added because reference to Interop.iTunesLib.dll is missing." unless references.has? "Interop.iTunesLib.dll"
		puts "Hotkeys Probe was not added because reference to ManagedWinapi.dll is missing." unless references.has? "ManagedWinapi.dll"
		puts "Websocket controller was not added because reference to Alchemy.dll is missing." unless references.has? "Alchemy.dll"
		
		for_compile = FileList["shutupify/**/*.cs"] 
		for_compile.exclude(/.*Lync.*.cs/i) 		unless references.has? "Microsoft.Lync.Model.dll", "Microsoft.Office.Uc.dll"
		for_compile.exclude(/.*itunes.*.cs/i) 		unless references.has? "Interop.iTunesLib.dll"
		for_compile.exclude(/.hotkeysprobe.cs/i) 	unless references.has? "ManagedWinapi.dll"
		for_compile.exclude(/.*websocket*.*.cs/i) 	unless references.has? "Alchemy.dll"
		for_compile.include(FOLDERS[:root] + "_AssemblyInfo.cs")
		
		puts "compiling #{for_compile.length} files."
		
		csc.use 		:net40
		csc.compile 	for_compile
		csc.references 	references
		csc.debug = debug
		csc.target = 	:library
		csc.output = 	libraryname

	end

	desc "builds the shutupify-app"
	csc :build_app => :build_lib do |csc|
		identify "build_app (as release: #{!debug})"

		application_name = FOLDERS[:build] + "shutupify-app.exe"
		File::delete application_name if File::exists? application_name
	
		references = FileList["#{FOLDERS[:build]}/shutupify-lib.dll"]
		references.extend(FileListEnhancement)
	
		for_compile = FileList["Application/**.cs"] 
		for_compile.include(FOLDERS[:root] + "_AssemblyInfo.cs")
		puts "compiling #{for_compile.length} files."
	
		csc.use 		:net40
		csc.compile 	for_compile
		csc.references 	references
		csc.debug = debug
		csc.target = 	:winexe
		csc.main =      "frm.Program"
		csc.output = 	application_name
	end

	desc "build the app"
	task :build => [:build_app, :clean_up]

	desc "build as release"
	task :as_release do 
		debug = false
	end


end