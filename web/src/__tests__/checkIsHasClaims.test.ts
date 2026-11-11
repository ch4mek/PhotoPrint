import './setupWindow';

import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserClaims } from '@/types/claims';

jest.mock('@/context/AuthContext');

const mockUseAuth = useAuth as jest.MockedFunction<typeof useAuth>;

describe('Helper function checkIsHasClaims', () => {
	it('should return false when claims is null', () => {
		mockUseAuth.mockReturnValue({ claims: null } as any);

		const result = checkIsHasClaims(
			[UserClaims.IsManager],
			mockUseAuth().claims
		);

		expect(result).toBe(false);
	});

	it('should return true when user has required claim', () => {
		mockUseAuth.mockReturnValue({
			claims: {
				[UserClaims.IsManager]: 'allowed',
				[UserClaims.ViewWiki]: 'denied',
			},
		} as any);

		const result = checkIsHasClaims(
			[UserClaims.IsManager],
			mockUseAuth().claims
		);

		expect(result).toBe(true);
	});

	it('should return false when user has no required claims', () => {
		mockUseAuth.mockReturnValue({
			claims: { [UserClaims.ViewWiki]: 'allowed' },
		} as any);

		const result = checkIsHasClaims(
			[UserClaims.IsManager, UserClaims.ManageArticle],
			mockUseAuth().claims
		);

		expect(result).toBe(false);
	});

	it('should return true when user has at least one required claim', () => {
		mockUseAuth.mockReturnValue({
			claims: { [UserClaims.ViewWiki]: 'allowed' },
		} as any);

		const result = checkIsHasClaims(
			[UserClaims.IsManager, UserClaims.ViewWiki],
			mockUseAuth().claims
		);

		expect(result).toBe(true);
	});
});
