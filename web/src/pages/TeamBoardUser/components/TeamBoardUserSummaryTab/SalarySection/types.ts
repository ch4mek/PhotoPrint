import { Dispatch, SetStateAction } from 'react';

import { UserSalaryPopUp } from '@/forms/TeamboardForm/Conversation/types';
import { TeamboardTableMode } from '@/types/teamboard';

export interface SalarySectionProps {
	setUserSalaries?: Dispatch<SetStateAction<UserSalaryPopUp[]>>;
	mode: TeamboardTableMode;
	userSalaries?: UserSalaryPopUp[];
	conversationId?: number | undefined;
}
