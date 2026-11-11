import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';

const MAX_BUTTON_SIZE = '24px';

export const ButtonWrapper = styled(Button)<{ isActive?: boolean }>(
	({ theme, isActive = false }) => ({
		color: isActive
			? theme.palette['graphics-accent']
			: theme.palette['text-black'],
		outline: 'none',
		border: 'none',
		padding: 0,
		display: 'inline-block',
		width: MAX_BUTTON_SIZE,
		height: MAX_BUTTON_SIZE,
		minWidth: 'auto',
		minHeight: 'auto',
		background: 'none !important',
	})
);
