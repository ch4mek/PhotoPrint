import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

const BUTTON_SIZE = 43;

export const ChatWidgetButtonWrapper = styled(ActionButton)({
	height: BUTTON_SIZE,
	width: BUTTON_SIZE,
});

export const ChatWidgetIcon = styled('img')({
	width: '100%',
});
