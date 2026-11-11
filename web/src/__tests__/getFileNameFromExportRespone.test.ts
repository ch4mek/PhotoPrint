import { getFileNameFromExportRespone } from '@/helpers/getFileNameFromExportResponse';

const testHeaderLines = [
	`attachment; filename=TimeReportSummary-19-02-2024.xlsx; filename*=UTF-8''TimeReportSummary-19-02-2024.xlsx`,
];

describe('Util function getFileNameFromExportRespone', () => {
	it('Should get correct file name from content-disposition header line', () => {
		expect(
			getFileNameFromExportRespone({
				headers: {
					'content-disposition': testHeaderLines[0],
				},
			})
		).toBe('TimeReportSummary-19-02-2024.xlsx');
	});
});
