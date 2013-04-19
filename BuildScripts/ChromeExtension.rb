
namespace :chrome do
	require 'coffee-script'


	desc "builds the chrome extension"
	task :build => :ensure_dirs do |task|
		identify task

		source_dir = FOLDERS[:root] + "ChromeExtension/"

		content = FileList[source_dir+"/Drivers/*.coffee", source_dir + "shutupify-content.coffee"]

		build_coffeescript FOLDERS[:chrome]+"shutupify.js", source_dir + "shutupify.coffee"
		build_coffeescript FOLDERS[:chrome]+"shutupify-content.js", content

		puts "\ncopying files to #{FOLDERS[:chrome]}"
		extension_files = FileList[source_dir+"manifest.json", source_dir+"icon.png"]
		extension_files.existing!
		cp extension_files, FOLDERS[:chrome] 

		version = File.read(FOLDERS[:root] + "VERSION")
		puts "\ntagging manifest.json with version #{version}"
		text = File.read(FOLDERS[:chrome] + "manifest.json")
		File.open(FOLDERS[:chrome] + "manifest.json", "w") {|file| file.puts text.gsub(/@VERSION@/, version)}
	end

	private 
	def build_coffeescript (target, *files)

		for_build = ensure_filelist files
		for_build.existing!

		puts "\ncompiling #{for_build.count} file/s into #{target}"

		coffee_script = for_build.to_ary.sort.map {|file|
			File.read(file)
			}.join("\n\n")

	# coffee_script.gsub! /.*console.log.*?\n/, ""

		java_script = CoffeeScript.compile coffee_script
	
		File.open(target, 'w') { |file| file.write java_script}
	end

end