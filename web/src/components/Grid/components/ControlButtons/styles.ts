import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';

export const ControlButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
}));

export const ControlButton = styled(Button)({
	flexShrink: 1,
});
