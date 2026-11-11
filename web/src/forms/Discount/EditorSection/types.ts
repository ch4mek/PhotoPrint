import { Dispatch, SetStateAction } from 'react';

import { CustomRootBlock, CustomValue } from '@/types/plateEditor';

export interface EditorSectionProps {
	isReadMode: boolean;
	initialData: CustomRootBlock[];
	setEditorData?: Dispatch<SetStateAction<CustomValue>>;
}
