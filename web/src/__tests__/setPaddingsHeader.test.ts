import { Routes } from '@/constants/routes';
import { setPaddingHeader } from '@/helpers/setPaddingHeader';

import { mockTheme } from './mocks/theme';

describe('Helper function setPaddingHeader', () => {
	const otherRouts = Object.values(Routes).filter(
		(value) => value !== '/sign-in'
	);

	it('Should return 0 padding for signIn route', () => {
		const result = setPaddingHeader('/sign-in', mockTheme);
		expect(result).toBe(0);
	});

	it.each(otherRouts)(
		'Should return default padding for other routes',
		(route) => {
			const result = setPaddingHeader(route, mockTheme);
			expect(result).toBe(mockTheme.spacing(8));
		}
	);
});
