import {
	createContext,
	ReactNode,
	useCallback,
	useContext,
	useMemo,
	useRef,
} from 'react';

import { CHAT_WIDGET_ID } from '@/context/ChatWidget/constants';
import { ChatWidgetContextValues } from '@/context/ChatWidget/types';
import { useShowModal } from '@/pages/hooks/useShowModal';

const ChatWidgetContext = createContext({} as ChatWidgetContextValues);

export function ChatWidgetProvider({ children }: { children: ReactNode }) {
	const { handleClose, handleOpen, isOpen, id } = useShowModal();
	const anchorElementRef = useRef<HTMLButtonElement | null>(null);
	const isChatOpen = isOpen && id === CHAT_WIDGET_ID;

	const handleOpenWidget = useCallback(() => handleOpen(CHAT_WIDGET_ID), []);

	const handleCloseWidget = useCallback(() => {
		if (isChatOpen) {
			handleClose();
		}
	}, [isChatOpen]);

	const handleToggleWidget = useCallback(() => {
		if (isChatOpen) {
			handleClose();
		} else {
			handleOpenWidget();
		}
	}, [isChatOpen]);

	const chatWidgetValue = useMemo(
		() => ({
			isOpen: isChatOpen,
			anchorElementRef,
			handleOpenWidget,
			handleCloseWidget,
			handleToggleWidget,
		}),
		[
			isChatOpen,
			anchorElementRef,
			handleOpenWidget,
			handleCloseWidget,
			handleToggleWidget,
		]
	);

	return (
		<ChatWidgetContext.Provider value={chatWidgetValue}>
			{children}
		</ChatWidgetContext.Provider>
	);
}

export const useChatWidget = () => useContext(ChatWidgetContext);
