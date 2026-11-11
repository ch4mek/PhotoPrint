import { getActivationMessage } from '@/helpers/getActivationMessage';

describe('Helper function getActivationMessage', () => {
	it('Should retun a string with project and deactivated words', () => {
		expect(
			getActivationMessage({ isActive: true, entityName: 'project' })
		).toBe(` project will be deactivated.`);
	});

	it('Should retun a string with project and activated words', () => {
		expect(
			getActivationMessage({ isActive: false, entityName: 'project' })
		).toBe(` project will be activated.`);
	});

	it('Should retun a string with client and deactivated words', () => {
		expect(getActivationMessage({ isActive: true, entityName: 'client' })).toBe(
			` client will be deactivated.`
		);
	});

	it('Should retun a string without entityName', () => {
		expect(getActivationMessage({ isActive: true, entityName: '' })).toBe(
			`  will be deactivated.`
		);
	});
});
