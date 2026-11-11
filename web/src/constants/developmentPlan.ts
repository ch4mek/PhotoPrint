import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { AllowedKeys, DevelopmentPlanStatusKeys } from '@/i18n/types';
import { OdataParams } from '@/types/common';
import {
	DevelopmentPlanFilters,
	DevelopmentPlanStatus,
} from '@/types/developmentPlan';

export const DEVELOPMENT_PLAN_SECTION_ROW_HEIGHT = 34;
export const DEVELOPMENT_PLAN_STATUS_TAG_WIDTH = 100;

export const DEVELOPMENT_PLAN_STATUS_CELL_CLASS_NAME =
	'development-plan-status-cell';

export const developmentStatusOptions: Array<SelectionModalOption> =
	Object.entries(DevelopmentPlanStatus).map(([key, value]) => ({
		id: key,
		description: value,
	}));

export const developmentPlanSectionDefaultStatusFilters = Object.keys(
	DevelopmentPlanStatus
).filter((key) => key !== DevelopmentPlanStatus.Approved);

export const developmentPlanSectionQueryParams: OdataParams<DevelopmentPlanFilters> =
	{
		skip: 0,
		top: 1000,
	};

export const DEADLINE_DATE_FIELD_NAME = 'deadlineDate';
export const START_DATE_FIELD_NAME = 'startDate';
export const STATUS_FIELD_NAME = 'status';
export const ID_FIELD_NAME = 'id';
export const DEFAULT_PLANS_ORDER_BY_FIELD = 'orderWeight';
export const DEVELOPMENT_PLAN_TITLE = 'Development plan';

export const developmentPlanStatus = {
	None: 'None',
	UnderReview: 'UnderReview',
	Approved: 'Approved',
	Rejected: 'Rejected',
} as const satisfies AllowedKeys<DevelopmentPlanStatusKeys>;
