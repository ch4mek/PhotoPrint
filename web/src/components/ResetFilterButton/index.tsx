import { Button } from '@mms/mms-ui-library/components/Button';
import { ResetFilterButton as ResetFilterButtonLib } from '@mms/mms-ui-library/components/ResetFilterButton';
import { memo } from 'react';

import { TextButtonWrapper } from './styles';
import { ResetFilterButtonProps } from './types';

function ResetFilterButtonComponent({
	text = 'Reset',
	disabled = false,
	withIcon = true,
	fullWidth = true,
	onClick,
}: ResetFilterButtonProps) {
	if (withIcon) {
		return <ResetFilterButtonLib size="big" {...{ disabled, onClick }} />;
	}

	return (
		<TextButtonWrapper fullWidth={fullWidth}>
			<Button
				isFullWidth={fullWidth}
				color="secondary"
				onClick={onClick}
				disabled={disabled}
			>
				{text}
			</Button>
		</TextButtonWrapper>
	);
}

export const ResetFilterButton = memo(ResetFilterButtonComponent);
