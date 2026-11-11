import chatWidgetIcon from '@/assets/common/chatWidgetIcon.png';
import {
	ChatWidgetButtonWrapper,
	ChatWidgetIcon,
} from '@/layouts/components/ChatWidgetButton/styles';
import { ChatWidgetButtonProps } from '@/layouts/components/ChatWidgetButton/types';

import {
	CHAT_WIDGET_ANCHOR_ELEMENT_ID,
	CHAT_WIDGET_ICON_ALT,
} from './constants';

export function ChatWidgetButton({ handleClick, ref }: ChatWidgetButtonProps) {
	return (
		<ChatWidgetButtonWrapper
			ref={ref}
			onClick={handleClick}
			id={CHAT_WIDGET_ANCHOR_ELEMENT_ID}
		>
			<ChatWidgetIcon alt={CHAT_WIDGET_ICON_ALT} src={chatWidgetIcon} />
		</ChatWidgetButtonWrapper>
	);
}
