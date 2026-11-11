import { ChatWidget } from '@/components/ChatWidget';
import { ChatWidgetRootProps } from '@/layouts/components/ChatWidgetRoot/types';

export function ChatWidgetRoot({
	isOpen,
	handleCloseWidget,
	ref,
}: ChatWidgetRootProps) {
	if (!isOpen) {
		return null;
	}

	return (
		<ChatWidget ref={ref} isOpen={isOpen} handleClose={handleCloseWidget} />
	);
}
