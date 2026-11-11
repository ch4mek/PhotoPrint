import {
	getDevelopmentStatusTagColor,
	getDevelopmentTagColor,
} from '@/helpers/getDevelopmentStatusTagColor';
import { GREEN_TAG_COLOR, RED_TAG_COLOR, YELLOW_TAG_COLOR } from '@/theme';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

describe('Helper function getDevelopmentStatusTagColor', () => {
	describe('getDevelopmentTagColor', () => {
		it('should return undefined for None or undefined values', () => {
			expect(
				getDevelopmentTagColor(DevelopmentPlanStatus.None)
			).toBeUndefined();
			expect(getDevelopmentTagColor(undefined)).toBeUndefined();
		});

		it('should return yellow for UnderReview status', () => {
			expect(getDevelopmentTagColor(DevelopmentPlanStatus.UnderReview)).toEqual(
				{
					backgroundColor: YELLOW_TAG_COLOR,
				}
			);
		});

		it('should return green for Approved status', () => {
			expect(getDevelopmentTagColor(DevelopmentPlanStatus.Approved)).toEqual({
				backgroundColor: GREEN_TAG_COLOR,
			});
		});

		it('should return red for Rejected status', () => {
			expect(getDevelopmentTagColor(DevelopmentPlanStatus.Rejected)).toEqual({
				backgroundColor: RED_TAG_COLOR,
			});
		});

		it('should return empty string for unknown status', () => {
			expect(getDevelopmentTagColor('UNKNOWN')).toEqual({
				backgroundColor: '',
			});
		});
	});

	describe('getDevelopmentStatusTagColor', () => {
		it('should return undefined if row.status is None or undefined', () => {
			expect(
				getDevelopmentStatusTagColor({ status: DevelopmentPlanStatus.None })
			).toBeUndefined();
			expect(
				getDevelopmentStatusTagColor({ status: undefined })
			).toBeUndefined();
		});

		it('should return correct color object based on row.status', () => {
			expect(
				getDevelopmentStatusTagColor({
					status: DevelopmentPlanStatus.UnderReview,
				})
			).toEqual({
				backgroundColor: YELLOW_TAG_COLOR,
			});
			expect(
				getDevelopmentStatusTagColor({ status: DevelopmentPlanStatus.Approved })
			).toEqual({
				backgroundColor: GREEN_TAG_COLOR,
			});
			expect(
				getDevelopmentStatusTagColor({ status: DevelopmentPlanStatus.Rejected })
			).toEqual({
				backgroundColor: RED_TAG_COLOR,
			});
		});
	});
});
