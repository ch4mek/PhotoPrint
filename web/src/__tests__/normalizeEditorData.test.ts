import { ELEMENT_PARAGRAPH } from '@udecode/plate';

import { normalizeEditorData } from '@/helpers/normalizeEditorData';
import { CustomRootBlock } from '@/types/plateEditor';

jest.mock('@udecode/plate', () => ({
	ELEMENT_PARAGRAPH: 'paragraph',
	isTextChild: (child: any) => typeof child?.text === 'string',
}));

describe('Helper function normalizeEditorData', () => {
	it('Should return empty array if input is undefined', () => {
		const result = normalizeEditorData(undefined);
		expect(result).toEqual([]);
	});

	it('Should normalize a block with single empty text child to empty string', () => {
		const block: CustomRootBlock = {
			type: ELEMENT_PARAGRAPH,
			children: [{ text: '   ' }],
			id: '1',
		};

		const result = normalizeEditorData([block]);
		expect(result[0].children[0].text).toBe('');
	});

	it('Should not modify blocks with non-empty text', () => {
		const block: CustomRootBlock = {
			type: ELEMENT_PARAGRAPH,
			children: [{ text: 'Hello' }],
			id: '2',
		};

		const result = normalizeEditorData([block]);
		expect(result[0].children[0].text).toBe('Hello');
	});

	it('Should not modify blocks with multiple children', () => {
		const block: CustomRootBlock = {
			type: ELEMENT_PARAGRAPH,
			children: [{ text: '   ' }, { text: 'Hello' }],
			id: '3',
		};

		const result = normalizeEditorData([block]);
		expect(result[0].children).toEqual(block.children);
	});

	it('Should preserve original block structure for blocks that do not match conditions', () => {
		const block: CustomRootBlock = {
			type: ELEMENT_PARAGRAPH,
			children: [{ text: 'Hello', bold: true }],
			id: '4',
		};

		const result = normalizeEditorData([block]);
		expect(result[0]).toEqual(block);
	});
});
