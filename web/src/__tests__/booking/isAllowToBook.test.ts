import { officeMock } from '../mocks/minskOffice';

import { MEETING_ROOMS_TAB_ID } from '@/constants/booking';
import { Roles } from '@/constants/roles';
import { isAllowToBook } from '@/helpers/booking';

type TestCase = [number, boolean];

const commonSpacesTestCases: Array<TestCase> = [
	[officeMock.workspaces[0].id, true],
	[MEETING_ROOMS_TAB_ID, true],
];

const jsSpacesTestCases: Array<TestCase> = [
	[officeMock.workspaces[0].id, true],
	[officeMock.workspaces[1].id, true],
	[officeMock.workspaces[2].id, true],
	[officeMock.workspaces[3].id, false],
	[MEETING_ROOMS_TAB_ID, true],
];

const javaSpacesTestCases: Array<TestCase> = [
	[officeMock.workspaces[0].id, true],
	[officeMock.workspaces[1].id, false],
	[officeMock.workspaces[2].id, false],
	[officeMock.workspaces[3].id, true],
	[MEETING_ROOMS_TAB_ID, true],
];

const anyDepartmentSpacesTestCases: Array<TestCase> = [
	[officeMock.workspaces[0].id, true],
	[officeMock.workspaces[1].id, false],
	[officeMock.workspaces[2].id, false],
	[officeMock.workspaces[3].id, false],
	[MEETING_ROOMS_TAB_ID, true],
];

describe('isAllowToBook tests', () => {
	it('should always return true for chief roles', () => {
		const isAllowForCeo = isAllowToBook(Roles.CEO, null, officeMock, null, '');
		const isAllowForCto = isAllowToBook(Roles.CTO, null, officeMock, null, '');

		expect(isAllowForCeo).toBe(true);
		expect(isAllowForCto).toBe(true);
	});

	it.each(commonSpacesTestCases)(
		'should return true for all roles for common spaces',
		(selectedOfficeId) => {
			const results = [
				isAllowToBook(Roles.CEO, null, officeMock, selectedOfficeId, ''),
				isAllowToBook(Roles.CTO, null, officeMock, selectedOfficeId, ''),
				isAllowToBook(Roles.HD, null, officeMock, selectedOfficeId, ''),
				isAllowToBook(Roles.SE, null, officeMock, selectedOfficeId, ''),
				isAllowToBook(Roles.SA, null, officeMock, selectedOfficeId, ''),
			];

			expect(results.includes(false)).toBe(false);
		}
	);

	it('should return true for any department for 403 space', () => {
		const selectedOfficeId = officeMock.workspaces[0].id;
		const results = [
			isAllowToBook(Roles.SE, null, officeMock, selectedOfficeId, ''),
			isAllowToBook(Roles.SE, 'Js', officeMock, selectedOfficeId, ''),
			isAllowToBook(Roles.SE, 'Java', officeMock, selectedOfficeId, ''),
			isAllowToBook(Roles.SE, 'QA', officeMock, selectedOfficeId, ''),
			isAllowToBook(Roles.SE, '.Net', officeMock, selectedOfficeId, ''),
			isAllowToBook(Roles.SE, 'PMO', officeMock, selectedOfficeId, ''),
		];

		expect(results.includes(false)).toBe(false);
	});

	it.each(jsSpacesTestCases)(
		'should return valid results for JS department',
		(selectedOfficeId, expectedResult) => {
			const departmentId = 'Js';

			const isAllowForHead = isAllowToBook(
				Roles.HD,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);
			const isAllowForEmployee = isAllowToBook(
				Roles.SE,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);

			expect(isAllowForHead).toBe(expectedResult);
			expect(isAllowForEmployee).toBe(expectedResult);
		}
	);

	it.each(javaSpacesTestCases)(
		'should return valid results for Java department',
		(selectedOfficeId, expectedResult) => {
			const departmentId = 'Java';

			const isAllowForHead = isAllowToBook(
				Roles.HD,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);
			const isAllowForEmployee = isAllowToBook(
				Roles.SE,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);

			expect(isAllowForHead).toBe(expectedResult);
			expect(isAllowForEmployee).toBe(expectedResult);
		}
	);

	it.each(anyDepartmentSpacesTestCases)(
		'should return valid results for any other department',
		(selectedOfficeId, expectedResult) => {
			const departmentId = 'NotJSOrJavaDepartment';

			const isAllowForHead = isAllowToBook(
				Roles.HD,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);
			const isAllowForEmployee = isAllowToBook(
				Roles.SE,
				departmentId,
				officeMock,
				selectedOfficeId,
				''
			);

			expect(isAllowForHead).toBe(expectedResult);
			expect(isAllowForEmployee).toBe(expectedResult);
		}
	);
});
