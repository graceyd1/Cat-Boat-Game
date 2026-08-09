#scons export options
# see https://popcar.bearblog.dev/how-to-minify-godots-build-size/
# and https://docs.godotengine.org/en/stable/engine_details/development/compiling/optimizing_for_size.html for more info

# scons platform=windows profile=custom.py

target="template_release"
disable_3d="yes"

# advanced text server (disabled) is used for special text features, other languages
module_text_server_adv_enabled="no" 
module_text_server_fb_enabled="yes"

deprecated="no"
winrt="no"

# vulcan used in Forward+ and Mobile renderers (project currently uses compatibility)
vulkan="no" 
use_volk="no"

openxr="no" 