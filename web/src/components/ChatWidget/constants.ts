import { spaces } from '@mms/mms-ui-library/theme';

import { CHAT_HEIGHT } from '@/components/ChatWidget/components/Chat/styles';

export const BORDER_WIDTH = 1;
const DESKTOP_LEFT_POSITION = 66;
export const DESKTOP_TRANSFORM = `translate(${DESKTOP_LEFT_POSITION}px, -${
	CHAT_HEIGHT + spaces.m + BORDER_WIDTH
}px)`;
