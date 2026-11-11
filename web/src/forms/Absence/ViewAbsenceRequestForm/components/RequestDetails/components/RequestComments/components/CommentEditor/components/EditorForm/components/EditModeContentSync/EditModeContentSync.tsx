import { usePlateEditorRef, usePlateId, usePlateStore } from '@udecode/plate';
import { useEffect } from 'react';

import { initialEditorData } from '@/constants/plateEditor';
import { CustomValue } from '@/types/plateEditor';

export function EditModeContentSync({
	editorData,
}: {
	editorData: CustomValue;
}) {
	const plateId = usePlateId();
	const editor = usePlateEditorRef(plateId ?? undefined);
	const store = usePlateStore(plateId ?? undefined);

	useEffect(() => {
		if (!editor || !store) {
			return;
		}

		const initialData: CustomValue =
			editorData.length > 0 ? editorData : initialEditorData;

		const deepCopiedData = JSON.parse(
			JSON.stringify(initialData)
		) as CustomValue;

		editor.children = deepCopiedData;
		store.set.value(deepCopiedData);

		const rerenderKey = 'keyEditor';
		store.set.incrementKey(rerenderKey);
	}, [editorData, editor, store]);

	return null;
}
