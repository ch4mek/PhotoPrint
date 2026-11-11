import {
	NOTE_MESSAGE,
	NOTE_MESSAGE_WITHOUT_DEVELOPMENT,
	NOTE_MESSAGE_WITHOUT_SALARY,
} from '../ConversationMessages';

export const getNoteMessage = (
	isGroupManagerRole: boolean,
	isIntern: boolean
) => {
	if (isGroupManagerRole) {
		return NOTE_MESSAGE_WITHOUT_SALARY;
	}

	if (isIntern) {
		return NOTE_MESSAGE_WITHOUT_DEVELOPMENT;
	}

	return NOTE_MESSAGE;
};
