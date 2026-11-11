import { CONFIRMATION_STARTING_ON_MESSAGE_PART } from '../constants';

import { DEVELOPMENT_PLAN_TITLE } from '@/constants/developmentPlan';

export const getDevelopmentPlanConformationMessage = (selectedDate: string) =>
	`${DEVELOPMENT_PLAN_TITLE} ${CONFIRMATION_STARTING_ON_MESSAGE_PART} ${selectedDate}`;
