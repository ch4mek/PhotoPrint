import { useTheme } from '@emotion/react';
import { HeadingToolbar, PlateEventProvider } from '@udecode/plate';
import { useMemo } from 'react';

import { ToolbarButtons } from '@/components/PlateToolbars/Toolbars';
import {
	getHeadingToolbarStyles,
	ToolbarButtonsWrapper,
} from '@/forms/Discount/EditorSection/components/EditorHeader/styles';

export function EditorHeader() {
	const theme = useTheme();

	const headingToolbarStyles = useMemo(
		() => getHeadingToolbarStyles(theme),
		[theme]
	);

	return (
		<PlateEventProvider>
			<HeadingToolbar
				styles={{
					root: headingToolbarStyles,
				}}
			>
				<ToolbarButtonsWrapper>
					<ToolbarButtons />
				</ToolbarButtonsWrapper>
			</HeadingToolbar>
		</PlateEventProvider>
	);
}
