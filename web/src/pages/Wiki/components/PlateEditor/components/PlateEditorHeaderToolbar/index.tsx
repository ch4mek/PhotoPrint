import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';
import { HeadingToolbar, PlateEventProvider } from '@udecode/plate';
import { useMemo } from 'react';

import { ToolbarButtons } from '@/components/PlateToolbars/Toolbars';
import { controlsText } from '@/constants/controlsTexts';
import {
	ButtonWrapper,
	getHeadingToolbarStyles,
	HeadingToolbarButtons,
	ToolbarButtonsWrapper,
} from '@/pages/Wiki/components/PlateEditor/components/PlateEditorHeaderToolbar/styles';
import { PlateEditorHeaderToolbarProps } from '@/pages/Wiki/components/PlateEditor/components/PlateEditorHeaderToolbar/types';

export function PlateEditorHeaderToolbar({
	handleReadButtonClick,
	handleSaveButtonClick,
}: PlateEditorHeaderToolbarProps) {
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
				<HeadingToolbarButtons>
					<Button
						onClick={handleReadButtonClick}
						isFullWidth={false}
						color="secondary"
					>
						{controlsText.cancelText}
					</Button>
					<ButtonWrapper>
						<Button onClick={handleSaveButtonClick}>
							{controlsText.saveText}
						</Button>
					</ButtonWrapper>
				</HeadingToolbarButtons>
			</HeadingToolbar>
		</PlateEventProvider>
	);
}
