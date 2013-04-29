
namespace :chrome do
	require 'coffee-script'

	CHROME = '"C:/Program Files (x86)/Google/Chrome/Application/chrome.exe"'
	@no_console_log = false

	desc "builds the chrome extension"
	task :build => :ensure_dirs do |task|
		identify task

		source_dir = FOLDERS[:root] + "ChromeExtension/"

		content = FileList[source_dir+"/Drivers/*.coffee", source_dir + "shutupify-content.coffee"]

		build_coffeescript FOLDERS[:chrome]+"shutupify.js", source_dir + "shutupify.coffee"
		build_coffeescript FOLDERS[:chrome]+"shutupify-content.js", content

		puts "\ncopying files to #{FOLDERS[:chrome]}"
		extension_files = FileList[source_dir+"manifest.json"]
		cp extension_files, FOLDERS[:chrome] 
		
		icon_files = FileList[source_dir+"Icon/*.png"]
		cp icon_files, FOLDERS[:chrome_assets] 

		version = File.read(FOLDERS[:root] + "VERSION")
		puts "\ntagging manifest.json with version #{version}"
		text = File.read(FOLDERS[:chrome] + "manifest.json")
		File.open(FOLDERS[:chrome] + "manifest.json", "w") {|file| file.puts text.gsub(/@VERSION@/, version)}
	end

	desc "pack chrome extension as crx file"
	task :pack  do |task|
		@no_console_log = true
		Rake::Task["chrome:build"].invoke
		
identify task

		source_dir = FOLDERS[:root] + "ChromeExtension/"
		fail "Could not find the key file at #{source_dir+"chrome.pem"}. CRX file could not be created." unless File::exists? source_dir+"chrome.pem"

		puts "\ncopying files to #{FOLDERS[:chrome]}"
		extension_files = FileList[source_dir+"chrome.pem"]
		extension_files.existing!
		cp extension_files, FOLDERS[:build] 

		puts "\npacking chrome extension as .crx"
		`#{CHROME} --pack-extension=#{FOLDERS[:chrome]} --pack-extension-key=#{FOLDERS[:build]}chrome.pem`
		
		rm "#{FOLDERS[:build]}chrome.pem"
	end

	private 
	def build_coffeescript (target, *files)

		for_build = ensure_filelist files
		for_build.existing!

		puts "\ncompiling #{for_build.count} file/s into #{target}"

		coffee_script = for_build.to_ary.sort.map {|file|
			File.read(file)
			}.join("\n\n")

		coffee_script.gsub! /.*console.log.*?\n/, "" if @no_console_log 

		java_script = CoffeeScript.compile coffee_script
	
		File.open(target, 'w') { |file| file.write java_script}
	end

end