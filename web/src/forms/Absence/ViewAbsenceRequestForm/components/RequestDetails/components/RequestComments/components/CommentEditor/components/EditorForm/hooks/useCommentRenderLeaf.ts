import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { renderLeaf } from '@/components/PlateToolbars/components/renderers/renderLeaf';
import { isEditorEmpty } from '@/helpers/plain';
import { CustomValue } from '@/types/plateEditor';

export function useCommentRenderLeaf(editorData: CustomValue) {
	const theme = useTheme();

	return useMemo(() => {
		const customColor = !isEditorEmpty(editorData)
			? `${theme.palette['text-black']} !important`
			: `${theme.palette['text-gray']} !important`;

		return renderLeaf(theme, customColor);
	}, [theme, editorData]);
}
