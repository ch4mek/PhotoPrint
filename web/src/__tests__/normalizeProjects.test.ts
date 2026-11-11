import { normalizeProjects } from '@/helpers/normalizeProjects';
import { ProjectLookupResponse } from '@/types/project';

describe('Helper function normalizeProjects', () => {
	const mockProjects: ProjectLookupResponse[] = [
		{
			id: 1,
			name: 'Project A',
			clientName: 'Client X',
			color: '#ff0000',
			isBillable: true,
			isCurrentUserAbleToSeeBillableTrackedTime: true,
		},
		{
			id: 2,
			name: 'Project B',
			clientName: 'Client Y',
			color: '#00ff00',
			isBillable: false,
			isCurrentUserAbleToSeeBillableTrackedTime: false,
		},
	];

	it('Should return empty array if isFetching is true', () => {
		const result = normalizeProjects(mockProjects, true)();
		expect(result).toEqual([]);
	});

	it('Should return empty array if projects is undefined', () => {
		const result = normalizeProjects(undefined, false)();
		expect(result).toEqual([]);
	});

	it('Should return empty array if projects array is empty', () => {
		const result = normalizeProjects([], false)();
		expect(result).toEqual([]);
	});

	it('Should normalize projects to only id and name', () => {
		const result = normalizeProjects(mockProjects, false)();
		expect(result).toEqual([
			{ id: 1, name: 'Project A' },
			{ id: 2, name: 'Project B' },
		]);
	});
});
