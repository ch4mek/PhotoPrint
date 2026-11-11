import { ConversationRow } from '@/pages/TeamBoardUser/types';

export interface ConversationViewPopUpProps {
	conversation: ConversationRow;
	handleClose: () => void;
}

export interface PropsWithConversationData {
	conversation: ConversationRow;
}
