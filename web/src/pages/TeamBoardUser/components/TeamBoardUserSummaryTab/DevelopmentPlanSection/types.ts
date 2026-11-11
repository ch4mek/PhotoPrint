import { Dispatch, SetStateAction } from 'react';

import { DevelopmentPlanPopUp } from '@/forms/TeamboardForm/Conversation/types';
import { TeamboardTableMode } from '@/types/teamboard';

export interface DevelopmentSectionProps {
	setDevelopmentPlans?: Dispatch<SetStateAction<DevelopmentPlanPopUp[]>>;
	mode: TeamboardTableMode;
	developmentPlans?: DevelopmentPlanPopUp[];
	conversationId?: number | undefined;
}
