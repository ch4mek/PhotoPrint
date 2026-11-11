import { ChatContent } from '@/components/ChatWidget/components/Chat/components/ChatContent';
import { ChatHeader } from '@/components/ChatWidget/components/Chat/components/ChatHeader';
import { ChatWrapper } from '@/components/ChatWidget/components/Chat/styles';
import { ChatProps } from '@/components/ChatWidget/components/types';
import { useResize } from '@/hooks/useResize';

export function Chat({ handleClose }: ChatProps) {
	const [, windowHeight] = useResize();

	return (
		<ChatWrapper windowHeight={windowHeight}>
			<ChatHeader handleClose={handleClose} />
			<ChatContent />
		</ChatWrapper>
	);
}
