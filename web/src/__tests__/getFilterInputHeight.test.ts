import { PAGE_HEADER_CONTROLS_HEIGHT } from '@/constants/commonStyles';
import { getFilterInputHeight } from '@/helpers/getFilterInputHeight';

describe('Helper function getFilterInputHeight', () => {
	it('should return height in px for desktop view', () => {
		expect(getFilterInputHeight(false)).toBe(
			`${PAGE_HEADER_CONTROLS_HEIGHT}px`
		);
	});

	it('should return undefined for mobile view', () => {
		expect(getFilterInputHeight(true)).toBeUndefined();
	});

	it('should return height in px if isMobileView is undefined', () => {
		expect(getFilterInputHeight(undefined)).toBe(
			`${PAGE_HEADER_CONTROLS_HEIGHT}px`
		);
	});
});
