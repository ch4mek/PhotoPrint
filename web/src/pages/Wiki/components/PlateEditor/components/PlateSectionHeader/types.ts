import { CustomValue } from '@/types/plateEditor';

export interface PlateSectionHeaderProps {
	title: string;
	setToggleEditorReadOnly: VoidFunction;
	editorData: CustomValue;
	isEditorEmpty: boolean;
}
