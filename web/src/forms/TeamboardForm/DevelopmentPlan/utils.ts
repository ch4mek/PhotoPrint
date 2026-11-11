import { Theme } from '@emotion/react';

import { popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import {
	DevelopmentPlanRow,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';

import { DevelopmentPlanFormValues } from './types';

export const convertDevelopmentPlanToFormInitialState = ({
	startDate,
	deadlineDate,
	notes,
	summary,
}: UserDevelopmentPlan | DevelopmentPlanRow): DevelopmentPlanFormValues => ({
	startDate: new Date(startDate),
	deadlineDate: deadlineDate ? new Date(deadlineDate) : null,
	notes,
	summary,
});

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.m,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
});
