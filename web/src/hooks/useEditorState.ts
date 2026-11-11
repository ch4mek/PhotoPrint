import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useState } from 'react';

import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { CustomValue } from '@/types/plateEditor';

export function useEditorState(initialValueFromProps: CustomValue) {
	const queryClient = useQueryClient();
	const [editorData, setEditorData] = useState<CustomValue>(
		initialValueFromProps
	);

	useEffect(() => {
		setEditorData(initialValueFromProps);
	}, [initialValueFromProps]);

	const resetEditor = useCallback(() => {
		queryClient.invalidateQueries({
			queryKey: [wikiQueriesKeys.getArticleById],
		});
	}, [queryClient]);

	return {
		editorData,
		setEditorData,
		resetEditor,
	};
}
