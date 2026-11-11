import styled from '@emotion/styled';
import { Button } from '@mms/mms-ui-library/components/Button';

const CONTROL_BUTTON_WIDTH = 87;

export const RequestControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
}));

export const ControlButton = styled(Button)({
	minWidth: CONTROL_BUTTON_WIDTH,
});
