import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';

import { ChatHeaderProps } from '@/components/ChatWidget/components/Chat/components/ChatHeader/types';

import { ChatHeaderInfo } from './components/ChatHeaderInfo';
import { ChatHeaderWrapper } from './styles';

export function ChatHeader({ handleClose }: ChatHeaderProps) {
	return (
		<ChatHeaderWrapper>
			<ChatHeaderInfo />
			<ActionButton onClick={handleClose}>
				<CrossIcon />
			</ActionButton>
		</ChatHeaderWrapper>
	);
}
