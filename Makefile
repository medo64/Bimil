#~ .NET Project

.SILENT:
.NOTPARALLEL:
.ONESHELL:

all clean run test benchmark examples tools debug release package publish ~clean ~run ~test ~benchmark ~examples ~tools ~debug ~release ~package ~publish &:
	./Make.sh $(MAKECMDGOALS)
