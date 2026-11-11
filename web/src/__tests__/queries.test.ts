import { getKeywordQuery } from '@/helpers/queries';

describe('Helper function getKeywordQuery', () => {
	it('Should return correct keyWord params', () => {
		expect(getKeywordQuery({ search: 'banana' })).toBe('?keyWord=banana');
	});

	it('Should return correct roles params', () => {
		expect(getKeywordQuery({ search: '', roles: ['CTO', 'CEO'] })).toBe(
			'?roles=CTO&roles=CEO'
		);
	});

	it('Should return correct isPublic params', () => {
		expect(getKeywordQuery({ search: '', roles: [], isPublic: false })).toBe(
			'?isPublic=false'
		);
	});

	it('Should return empty string if input is null', () => {
		expect(getKeywordQuery({})).toBe('');
	});

	it('Should return empty string if input is empty string', () => {
		expect(getKeywordQuery({ search: '' })).toBe('');
	});

	it('Should return isActive param if was provided', () => {
		expect(getKeywordQuery({ isActive: true })).toBe('?isActive=true');
	});

	it('Should return isDeleted param if was provided', () => {
		expect(getKeywordQuery({ isDeleted: true })).toBe('?isDeleted=true');
	});
});
