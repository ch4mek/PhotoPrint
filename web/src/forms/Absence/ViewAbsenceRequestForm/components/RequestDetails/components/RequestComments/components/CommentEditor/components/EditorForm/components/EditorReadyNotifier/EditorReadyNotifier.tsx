import { usePlateEditorRef, usePlateId } from '@udecode/plate';
import { useEffect } from 'react';

import { EditorReadyNotifierProps } from '../../types';

import { CustomEditor } from '@/types/plateEditor';

export function EditorReadyNotifier({ onReady }: EditorReadyNotifierProps) {
	const plateId = usePlateId();
	const editor = usePlateEditorRef(plateId ?? undefined);

	useEffect(() => {
		if (editor) {
			onReady(editor as CustomEditor);
		}
	}, [editor, onReady]);

	return null;
}
