.PHONY: all clean distclean dist debug release

all: release

base:
	@chmod +x ./Make.sh

clean: base
	@./Make.sh clean

distclean: base
	@./Make.sh distclean

dist: base
	@./Make.sh dist

debug: base
	@./Make.sh debug

release: base
	@./Make.sh release
