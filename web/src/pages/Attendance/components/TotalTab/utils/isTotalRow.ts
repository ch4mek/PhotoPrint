import { AttendanceTotalGridRow } from '../types';

export const isAttendanceTotalRow = (
	row: unknown
): row is AttendanceTotalGridRow =>
	typeof row === 'object' && row !== null && 'totalTime' in row;
