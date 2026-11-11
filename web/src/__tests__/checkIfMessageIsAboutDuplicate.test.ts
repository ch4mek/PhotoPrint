import {
	checkIfMessageIsAboutEmailDuplicate,
	checkIfMessageIsAboutPhoneDuplicate,
	checkIfMessageIsAboutRoleDuplicate,
} from '@/helpers/checkIfMessageIsAboutDuplicate';

describe('Helper function checkIfMessageIsAboutRoleDuplicate', () => {
	const falsyRoleMessages = [
		['Wrong message'],
		['A User with CEO role has wrong end'],
		['Wrong begin of a user with CEO role is already existed'],
	];
	const truthyRoleMessages = [
		['A User with CEO role is already existed.'],
		['A User with CTO role is already existed.'],
		['A User with HeadOfDepartment role is already existed.'],
	];

	test.each(falsyRoleMessages)(
		"Should return false if message not includes 'A User with' and (or) not includes 'role is already existed.'",
		(message) => {
			expect(checkIfMessageIsAboutRoleDuplicate(message)).toBe(false);
		}
	);

	test.each(truthyRoleMessages)(
		"Should return true if message includes 'A User with' and includes 'role is already existed.'",
		(message) => {
			expect(checkIfMessageIsAboutRoleDuplicate(message)).toBe(true);
		}
	);
});

describe('Helper function checkIfMessageIsAboutPhoneDuplicate', () => {
	const falsyPhoneMessages = ['Invalid phone number', 'Error', ''];

	const truthyPhoneMessages = [
		'This phone number is already used by',
		'This phone number is already used by another user',
		'Error: This phone number is already used by account user with id 123',
	];

	test.each(falsyPhoneMessages)(
		'Should return false when message does not contain "This phone number is already used by"',
		(message) => {
			expect(checkIfMessageIsAboutPhoneDuplicate(message)).toBe(false);
		}
	);

	test.each(truthyPhoneMessages)(
		'Should return true when message contains "This phone number is already used by"',
		(message) => {
			expect(checkIfMessageIsAboutPhoneDuplicate(message)).toBe(true);
		}
	);
});

describe('Helper function checkIfMessageIsAboutEmailDuplicate', () => {
	test('Should return true for duplicate email message', () => {
		expect(checkIfMessageIsAboutEmailDuplicate('Email is already used')).toBe(
			true
		);
	});

	test('Should return true for message containing detailed info', () => {
		expect(
			checkIfMessageIsAboutEmailDuplicate(
				'Error: Email test@mail.com is already used by test user'
			)
		).toBe(true);
	});

	test('Should return true for message about personal email', () => {
		expect(
			checkIfMessageIsAboutEmailDuplicate('Personal email is already used')
		).toBe(true);
	});

	test('Should return false for different message', () => {
		expect(checkIfMessageIsAboutEmailDuplicate('Invalid password')).toBe(false);
	});

	test('Should return false for empty string', () => {
		expect(checkIfMessageIsAboutEmailDuplicate('')).toBe(false);
	});
});
