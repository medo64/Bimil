#~ .NET Project

.SILENT:
.NOTPARALLEL:
.ONESHELL:

.PHONY: all clean run test benchmark examples tools debug release package publish ~clean ~run ~test ~benchmark ~examples ~tools ~debug ~release ~package ~publish

all clean run test benchmark examples tools debug release package publish ~clean ~run ~test ~benchmark ~examples ~tools ~debug ~release ~package ~publish &:
	./Make.sh $(MAKECMDGOALS)
