import { getPlateEditorRef, insertText } from '@udecode/plate';
import { KeyboardEvent } from 'react';

import { CONFIG } from '@/plugins/PlateEditorPlugins/config';

export const handleEditorKeyDown = (
	e: KeyboardEvent<HTMLDivElement>,
	editorId: string
) => {
	if (e.key === 'Tab') {
		e.preventDefault();
		e.stopPropagation();

		const editor = getPlateEditorRef(editorId);

		if (editor) {
			insertText(editor, '\t');
		}
	}
	CONFIG.editableProps.onKeyDown?.(e);
};
