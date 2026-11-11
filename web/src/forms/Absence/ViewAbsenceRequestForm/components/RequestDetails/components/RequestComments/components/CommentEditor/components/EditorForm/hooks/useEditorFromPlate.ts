import { useCallback, useEffect, useState } from 'react';

import { CustomEditor } from '@/types/plateEditor';

export function useEditorFromPlate(editorId: string) {
	const [editor, setEditor] = useState<CustomEditor | null>(null);

	const onEditorReady = useCallback((editorInstance: CustomEditor) => {
		setEditor(editorInstance);
	}, []);

	useEffect(() => {
		setEditor(null);
	}, [editorId]);

	return { editor, onEditorReady };
}
