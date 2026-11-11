import { Roles } from '@/constants/roles';
import { checkIfFirstEntryProfileRoute } from '@/helpers/checkIfFirstEntryProfileRoute';
import { UserInfo } from '@/types/userInfo';

const user1: UserInfo = {
	id: 1,
	firstName: 'string',
	lastName: 'string',
	color: 'string',
	email: 'string',
	timeZone: 'string',
	position: Roles.HD,
	rightPosition: 'string',
	departmentId: 'Js',
	location: 'string',
	avatarFileId: 'string',
};

const user2: UserInfo = {
	id: 2,
	firstName: 'string',
	lastName: 'string',
	color: 'string',
	email: 'string',
	timeZone: 'string',
	position: Roles.SE,
	rightPosition: 'string',
	departmentId: 'Js',
	location: 'string',
	avatarFileId: 'string',
};

const user3: UserInfo = {
	id: 3,
	firstName: 'string',
	lastName: 'string',
	color: 'string',
	email: 'string',
	timeZone: 'string',
	position: Roles.SE,
	rightPosition: 'string',
	departmentId: 'Js',
	location: '',
	avatarFileId: 'string',
};

describe('Validation function checkIfFirstEntryProfileRoute', () => {
	it("Should return false if user isn\\'t authorized and he is chief", () => {
		expect(checkIfFirstEntryProfileRoute(user1, false)).toBe(false);
	});

	it("Should return false if user isn\\'t authorized and he isn\\'t chief", () => {
		expect(checkIfFirstEntryProfileRoute(user2, false)).toBe(false);
	});

	it("Should return false if user isn\\'t authorized and he isn\\'t chief and he hasn\\'t location", () => {
		expect(checkIfFirstEntryProfileRoute(user3, false)).toBe(false);
	});

	it('Should return false if user is authorized and he is chief and he has location', () => {
		expect(checkIfFirstEntryProfileRoute(user1, true)).toBe(false);
	});

	it("Should return false if user is authorized and he isn\\'t chief and he has location", () => {
		expect(checkIfFirstEntryProfileRoute(user2, true)).toBe(false);
	});

	it("Should return true if user is authorized and he isn\\'t chief and he hasn\\'t location", () => {
		expect(checkIfFirstEntryProfileRoute(user3, true)).toBe(true);
	});
});
