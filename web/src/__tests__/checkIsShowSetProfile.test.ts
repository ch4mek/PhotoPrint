import { Roles } from '@/constants/roles';
import { checkIsShowSetProfile } from '@/helpers/checkIsShowSetProfile';

describe('Helper function checkIsShowSetProfile', () => {
	describe('when position is in chiefRoles', () => {
		it('should return true if location is empty', () => {
			expect(checkIsShowSetProfile('', '', Roles.CEO)).toBe(true);
		});

		it('should return false if location is provided', () => {
			expect(checkIsShowSetProfile('NY', '', Roles.CTO)).toBe(false);
		});
	});

	describe('when position is not in chiefRoles', () => {
		it('should return true if location is empty', () => {
			expect(checkIsShowSetProfile('', '123', Roles.SA)).toBe(true);
		});

		it('should return true if departmentId is empty', () => {
			expect(checkIsShowSetProfile('NY', '', Roles.SE)).toBe(true);
		});

		it('should return false if both location and departmentId are provided', () => {
			expect(checkIsShowSetProfile('NY', '123', Roles.HD)).toBe(false);
		});
	});
});
