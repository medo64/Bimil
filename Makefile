.PHONY: all clean distclean dist debug release run test

all:
	@bash ./Make.sh release

clean:
	@bash ./Make.sh clean

distclean:
	@bash ./Make.sh distclean

dist:
	@bash ./Make.sh dist

debug:
	@bash ./Make.sh debug

release:
	@bash ./Make.sh release

run:
	@bash ./Make.sh run

test:
	@bash ./Make.sh test
