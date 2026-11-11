import { TagStyles } from '@/components/Grid/renderers/cell/tagCellRenderer/types';
import { GREEN_TAG_COLOR, RED_TAG_COLOR, YELLOW_TAG_COLOR } from '@/theme';
import {
	DevelopmentPlanRow,
	DevelopmentPlanStatus,
} from '@/types/developmentPlan';

export const getDevelopmentTagColor = <T>(value?: T | string) => {
	let backgroundColor: string = '';

	if (value === DevelopmentPlanStatus.None || !value) {
		return undefined;
	}

	if (value === DevelopmentPlanStatus.UnderReview) {
		backgroundColor = YELLOW_TAG_COLOR;
	}

	if (value === DevelopmentPlanStatus.Approved) {
		backgroundColor = GREEN_TAG_COLOR;
	}

	if (value === DevelopmentPlanStatus.Rejected) {
		backgroundColor = RED_TAG_COLOR;
	}

	return {
		backgroundColor,
	};
};

export const getDevelopmentStatusTagColor = (
	row: Partial<DevelopmentPlanRow>
): TagStyles | undefined => getDevelopmentTagColor(row.status);
