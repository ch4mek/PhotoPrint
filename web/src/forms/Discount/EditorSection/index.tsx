import { useTheme } from '@emotion/react';
import { Plate } from '@udecode/plate';
import { useMemo } from 'react';

import { renderLeaf } from '@/components/PlateToolbars/components/renderers/renderLeaf';
import { getDefaultColor, EDITOR_ID } from '@/constants/plateEditor';
import { DESCRIPTION_TITLE } from '@/forms/Discount/DiscountForm/constants';
import { EditorHeader } from '@/forms/Discount/EditorSection/components/EditorHeader';
import {
	EditorSectionWrapper,
	PlateWrapper,
} from '@/forms/Discount/EditorSection/styles';
import { EditorSectionProps } from '@/forms/Discount/EditorSection/types';
import { handleEditorKeyDown } from '@/helpers/handleEditorKeyDown';
import { getCustomPluginsWithThemeMode } from '@/plugins/PlateEditorPlugins';
import { CONFIG } from '@/plugins/PlateEditorPlugins/config';
import { CustomValue } from '@/types/plateEditor';

export function EditorSection({
	isReadMode,
	initialData,
	setEditorData,
}: EditorSectionProps) {
	const theme = useTheme();
	const defaultColor = getDefaultColor(theme.mode);
	const customPlugins = getCustomPluginsWithThemeMode(theme.mode);

	const handleEditorChange = (value: CustomValue) => {
		setEditorData?.(value);
	};

	const renderLeafNode = useMemo(
		() => renderLeaf(theme, defaultColor),
		[theme, defaultColor]
	);

	return (
		<EditorSectionWrapper>
			{!isReadMode && <EditorHeader />}
			<PlateWrapper isReadMode={isReadMode}>
				<Plate<CustomValue>
					id={EDITOR_ID}
					editableProps={{
						...CONFIG.editableProps,
						readOnly: isReadMode,
						placeholder: !isReadMode ? DESCRIPTION_TITLE : '',
						onKeyDown: (e) => handleEditorKeyDown(e, EDITOR_ID),
						renderLeaf: renderLeafNode,
						style: {
							...CONFIG.editableProps.style,
							color: defaultColor,
						},
					}}
					initialValue={initialData}
					onChange={handleEditorChange}
					plugins={customPlugins}
				/>
			</PlateWrapper>
		</EditorSectionWrapper>
	);
}
