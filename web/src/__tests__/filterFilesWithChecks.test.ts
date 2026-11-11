import {
	FileCheck,
	filterFilesWithChecks,
} from '@/helpers/filterFilesWithChecks';

describe('Helper function filterFilesWithChecks', () => {
	const file1 = {
		name: 'file1.txt',
		type: 'text/plain',
		size: 100,
	} as unknown as File;
	const file2 = {
		name: 'file2.txt',
		type: 'text/plain',
		size: 200,
	} as unknown as File;
	const uploadedFiles = [file1, file2];

	let mockFileCheckPass: FileCheck;
	let mockFileCheckFail: FileCheck;

	beforeEach(() => {
		mockFileCheckPass = {
			displayError: jest.fn(),
			patchFiles: jest.fn((files) => files.map((file) => file)),
			checkFiles: jest.fn(() => true),
		};

		mockFileCheckFail = {
			displayError: jest.fn(),
			patchFiles: jest.fn(() => []),
			checkFiles: jest.fn(() => false),
		};
	});

	it('Should return the same files if no checks fail', () => {
		const result = filterFilesWithChecks(uploadedFiles, [mockFileCheckFail]);
		expect(result).toEqual(uploadedFiles);
		expect(mockFileCheckFail.displayError).not.toHaveBeenCalled();
		expect(mockFileCheckFail.patchFiles).not.toHaveBeenCalled();
	});

	it('Should call displayError and patchFiles if checkFiles returns true', () => {
		const result = filterFilesWithChecks(uploadedFiles, [mockFileCheckPass]);
		expect(result).toEqual(uploadedFiles);
		expect(mockFileCheckPass.displayError).toHaveBeenCalledTimes(1);
		expect(mockFileCheckPass.patchFiles).toHaveBeenCalledWith(uploadedFiles);
	});

	it('Should apply multiple FileChecks in sequence', () => {
		const secondCheck: FileCheck = {
			displayError: jest.fn(),
			patchFiles: jest.fn((files) => files.slice(0, 1)),
			checkFiles: jest.fn(() => true),
		};

		const result = filterFilesWithChecks(uploadedFiles, [
			mockFileCheckPass,
			secondCheck,
		]);
		expect(result).toEqual([file1]);
		expect(mockFileCheckPass.displayError).toHaveBeenCalled();
		expect(secondCheck.displayError).toHaveBeenCalled();
		expect(secondCheck.patchFiles).toHaveBeenCalled();
	});

	it('Should return empty array if all files are removed by patchFiles', () => {
		const removeAllCheck: FileCheck = {
			displayError: jest.fn(),
			patchFiles: jest.fn(() => []),
			checkFiles: jest.fn(() => true),
		};

		const result = filterFilesWithChecks(uploadedFiles, [removeAllCheck]);
		expect(result).toEqual([]);
		expect(removeAllCheck.displayError).toHaveBeenCalled();
	});

	it('Should handle empty uploadedFiles array', () => {
		const result = filterFilesWithChecks([], [mockFileCheckPass]);
		expect(result).toEqual([]);
		expect(mockFileCheckPass.displayError).not.toHaveBeenCalled();
		expect(mockFileCheckPass.patchFiles).not.toHaveBeenCalled();
	});
});
