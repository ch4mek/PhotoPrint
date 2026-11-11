import { useCallback } from 'react';

import { ChatWidgetProvider, useChatWidget } from '@/context/ChatWidget';
import { ChatWidgetButton } from '@/layouts/components/ChatWidgetButton';
import { ChatWidgetRoot } from '@/layouts/components/ChatWidgetRoot';

function HrChatInner() {
	const { anchorElementRef, handleToggleWidget, handleCloseWidget, isOpen } =
		useChatWidget();

	const handleClick = useCallback(() => {
		handleToggleWidget();
	}, [handleToggleWidget]);

	return (
		<>
			<ChatWidgetButton ref={anchorElementRef} handleClick={handleClick} />
			<ChatWidgetRoot
				ref={anchorElementRef}
				{...{ isOpen, handleCloseWidget }}
			/>
		</>
	);
}
export function HrChat() {
	return (
		<ChatWidgetProvider>
			<HrChatInner />
		</ChatWidgetProvider>
	);
}
