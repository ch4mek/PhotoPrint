import { Button } from '@mms/mms-ui-library/components/Button';
import { TeamsIcon } from '@mms/mms-ui-library/icons/base';

import { TEAMS_CHAT_BASE_URL, TEAMS_CHAT_BUTTON_TEXT } from './constants';
import { TeamsChatButtonProps } from './types';

export const TeamsChatButton = ({ email }: TeamsChatButtonProps) => {
	const handleClick = () => {
		window.open(`${TEAMS_CHAT_BASE_URL}?users=${email}`, '_blank', 'noopener');
	};

	return (
		<Button
			color="secondary"
			onClick={handleClick}
			addon={<TeamsIcon />}
			isFullWidth={false}
		>
			{TEAMS_CHAT_BUTTON_TEXT}
		</Button>
	);
};
