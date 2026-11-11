import { SortDirection } from '@/constants/sort';
import { updateGridItems } from '@/helpers/updateGridItems';

const testData = {
	items: [
		{
			color: 'red',
			comments: '1',
			id: 1,
			isDeleted: false,
			isProjectExist: true,
			name: 'Andrew Tate',
			trackedTime: 10,
		},
		{
			color: 'blue',
			comments: 'b',
			id: 2,
			isDeleted: false,
			isProjectExist: true,
			name: 'Bill Gates',
			trackedTime: 100,
		},
		{
			color: 'green',
			comments: 'c',
			id: 3,
			isDeleted: false,
			isProjectExist: true,
			name: 'Elon Musk',
			trackedTime: 1000,
		},
		{
			color: 'green',
			comments: 'c',
			id: 4,
			isDeleted: false,
			isProjectExist: true,
			name: 'Will Smith',
			trackedTime: 1001,
		},
	],
};

describe('Helper function updateGridItems', () => {
	it('Should return array with updated item and sorted by name in descending order (default)', () => {
		const filteredByName = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 100,
			},
			'name'
		);
		expect(filteredByName.items).toHaveLength(4);
		expect(filteredByName.items[0].name).toBe('Will Smith');
		expect(filteredByName.items[1].name).toBe('Elon Musk');
		expect(filteredByName.items[2].name).toBe('Bill Gates');
		expect(filteredByName.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by time in descending order (default)', () => {
		const filteredByTime = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'trackedTime'
		);
		expect(filteredByTime.items).toHaveLength(4);
		expect(filteredByTime.items[0].name).toBe('Bill Gates');
		expect(filteredByTime.items[0].trackedTime).toBe(10000);
		expect(filteredByTime.items[1].name).toBe('Will Smith');
		expect(filteredByTime.items[2].name).toBe('Elon Musk');
		expect(filteredByTime.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by color in descending order (default)', () => {
		const filteredByColor = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'color'
		);
		expect(filteredByColor.items).toHaveLength(4);
		expect(filteredByColor.items[0].name).toBe('Andrew Tate');
		expect(filteredByColor.items[1].name).toBe('Bill Gates');
		expect(filteredByColor.items[1].color).toBe('purple');
		expect(filteredByColor.items[2].name).toBe('Elon Musk');
		expect(filteredByColor.items[3].name).toBe('Will Smith');
	});

	it('Should return array with updated item and sorted by Comment in descending order (default)', () => {
		const filteredByComment = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: 'test',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'comments'
		);
		expect(filteredByComment.items).toHaveLength(4);
		expect(filteredByComment.items[0].name).toBe('Bill Gates');
		expect(filteredByComment.items[0].comments).toBe('test');
		expect(filteredByComment.items[1].name).toBe('Elon Musk');
		expect(filteredByComment.items[2].name).toBe('Will Smith');
		expect(filteredByComment.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by name in descending order', () => {
		const filteredByName = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 100,
			},
			'name',
			SortDirection.DESC
		);
		expect(filteredByName.items).toHaveLength(4);
		expect(filteredByName.items[0].name).toBe('Will Smith');
		expect(filteredByName.items[1].name).toBe('Elon Musk');
		expect(filteredByName.items[2].name).toBe('Bill Gates');
		expect(filteredByName.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by time in descending order', () => {
		const filteredByTime = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'trackedTime',
			SortDirection.DESC
		);
		expect(filteredByTime.items).toHaveLength(4);
		expect(filteredByTime.items[0].name).toBe('Bill Gates');
		expect(filteredByTime.items[0].trackedTime).toBe(10000);
		expect(filteredByTime.items[1].name).toBe('Will Smith');
		expect(filteredByTime.items[2].name).toBe('Elon Musk');
		expect(filteredByTime.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by color in descending order', () => {
		const filteredByColor = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'color',
			SortDirection.DESC
		);
		expect(filteredByColor.items).toHaveLength(4);
		expect(filteredByColor.items[0].name).toBe('Andrew Tate');
		expect(filteredByColor.items[1].name).toBe('Bill Gates');
		expect(filteredByColor.items[1].color).toBe('purple');
		expect(filteredByColor.items[2].name).toBe('Elon Musk');
		expect(filteredByColor.items[3].name).toBe('Will Smith');
	});

	it('Should return array with updated item and sorted by Comment in descending order', () => {
		const filteredByComment = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: 'test',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'comments',
			SortDirection.DESC
		);
		expect(filteredByComment.items).toHaveLength(4);
		expect(filteredByComment.items[0].name).toBe('Bill Gates');
		expect(filteredByComment.items[0].comments).toBe('test');
		expect(filteredByComment.items[1].name).toBe('Elon Musk');
		expect(filteredByComment.items[2].name).toBe('Will Smith');
		expect(filteredByComment.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by name in ascending order', () => {
		const filteredByName = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 100,
			},
			'name',
			SortDirection.ASC
		);
		expect(filteredByName.items).toHaveLength(4);
		expect(filteredByName.items[0].name).toBe('Andrew Tate');
		expect(filteredByName.items[1].name).toBe('Bill Gates');
		expect(filteredByName.items[2].name).toBe('Elon Musk');
		expect(filteredByName.items[3].name).toBe('Will Smith');
	});

	it('Should return array with updated item and sorted by time in ascending order', () => {
		const filteredByTime = updateGridItems(
			testData,
			{
				color: '',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'trackedTime',
			SortDirection.ASC
		);
		expect(filteredByTime.items).toHaveLength(4);
		expect(filteredByTime.items[0].name).toBe('Andrew Tate');
		expect(filteredByTime.items[1].name).toBe('Elon Musk');
		expect(filteredByTime.items[2].name).toBe('Will Smith');
		expect(filteredByTime.items[3].name).toBe('Bill Gates');
		expect(filteredByTime.items[3].trackedTime).toBe(10000);
	});

	it('Should return array with updated item and sorted by color in ascending order', () => {
		const filteredByColor = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: '',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'color',
			SortDirection.ASC
		);
		expect(filteredByColor.items).toHaveLength(4);
		expect(filteredByColor.items[0].name).toBe('Elon Musk');
		expect(filteredByColor.items[1].name).toBe('Will Smith');
		expect(filteredByColor.items[2].name).toBe('Bill Gates');
		expect(filteredByColor.items[2].color).toBe('purple');
		expect(filteredByColor.items[3].name).toBe('Andrew Tate');
	});

	it('Should return array with updated item and sorted by comment in ascending order', () => {
		const filteredByComment = updateGridItems(
			testData,
			{
				color: 'purple',
				comments: 'test',
				id: 2,
				isDeleted: false,
				isProjectExist: true,
				name: 'Bill Gates',
				trackedTime: 10000,
			},
			'comments',
			SortDirection.ASC
		);
		expect(filteredByComment.items).toHaveLength(4);
		expect(filteredByComment.items[0].name).toBe('Andrew Tate');
		expect(filteredByComment.items[1].name).toBe('Elon Musk');
		expect(filteredByComment.items[2].name).toBe('Will Smith');
		expect(filteredByComment.items[3].name).toBe('Bill Gates');
		expect(filteredByComment.items[3].comments).toBe('test');
	});

	it('Should return empty array if input undefined', () => {
		expect(updateGridItems(undefined, undefined, '')).toEqual({
			items: [],
		});
	});
});
