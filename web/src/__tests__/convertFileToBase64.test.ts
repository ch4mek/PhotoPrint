import { convertFileToBase64 } from '@/helpers/convertFileToBase64';

const ERROR_MESSAGE = 'File reading failed';

describe('convertFileToBase64', () => {
	let fileReaderMock: Partial<FileReader>;

	beforeEach(() => {
		fileReaderMock = {
			readAsDataURL: jest.fn(),
			addEventListener: jest.fn(),
		};

		global.FileReader = jest.fn(() => fileReaderMock as FileReader);

		global.File = class {
			constructor(
				public parts: (BlobPart | string)[],
				public name: string,
				public options: { type: string }
			) {}
		} as any;
	});

	it('should resolve with base64 string when reading succeeds', async () => {
		const mockFile = new File(['hello'], 'hello.png', { type: 'image/png' });
		const expectedResult = 'data:image/png;base64,aGVsbG8=';

		const readerInstance = {
			readAsDataURL: jest.fn(),
			onloadend: null as null | (() => void),
			onerror: null as null | (() => void),
			result: expectedResult,
		};

		jest
			.spyOn(global, 'FileReader')
			.mockImplementation(() => readerInstance as any);

		const promise = convertFileToBase64(mockFile);

		readerInstance.onloadend?.();

		await expect(promise).resolves.toBe(expectedResult);
	});

	it('should reject when reading fails', async () => {
		const mockFile = new File(['hello'], 'hello.png', { type: 'image/png' });

		const readerInstance = {
			readAsDataURL: jest.fn(),
			onloadend: null,
			onerror: null as null | ((e: any) => void),
		};

		jest
			.spyOn(global, 'FileReader')
			.mockImplementation(() => readerInstance as any);

		const promise = convertFileToBase64(mockFile);

		const mockError = new Error(ERROR_MESSAGE);
		readerInstance.onerror?.(mockError);

		await expect(promise).rejects.toThrow(ERROR_MESSAGE);
	});
});
