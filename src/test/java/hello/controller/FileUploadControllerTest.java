package hello.controller;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.DisplayName;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.mock.web.MockMultipartFile;
import org.springframework.test.context.TestPropertySource;
import org.springframework.test.web.servlet.MockMvc;

import static org.junit.jupiter.api.Assertions.*;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.multipart;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

/**
 * Test class for FileUploadController to improve code coverage.
 * Focuses on testing exception handlers and error paths.
 */
@SpringBootTest
@AutoConfigureMockMvc
@TestPropertySource(properties = {
    "spring.servlet.multipart.max-file-size=1KB",
    "spring.servlet.multipart.max-request-size=1KB"
})
class FileUploadControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Test
    @DisplayName("Upload file with valid size should succeed")
    void testUploadFile_Success() throws Exception {
        // Create a small file (under 1KB limit)
        byte[] content = new byte[512];
        for (int i = 0; i < content.length; i++) {
            content[i] = (byte) (i % 256);
        }
        
        MockMultipartFile file = new MockMultipartFile(
            "file",
            "test.txt",
            "text/plain",
            content
        );

        mockMvc.perform(multipart("/api/v1/files/upload")
                .file(file))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.message").value("File uploaded successfully"))
            .andExpect(jsonPath("$.filename").value("test.txt"))
            .andExpect(jsonPath("$.size").exists());
    }

    @Test
    @DisplayName("Exception handler should handle MaxUploadSizeExceededException")
    void testHandleMaxUploadSizeExceeded() {
        // Directly test the exception handler method
        FileUploadController controller = new FileUploadController();
        org.springframework.web.multipart.MaxUploadSizeExceededException ex = 
            new org.springframework.web.multipart.MaxUploadSizeExceededException(1024);
        
        var response = controller.handleMaxUploadSizeExceeded(ex);
        
        assertEquals(413, response.getStatusCode().value());
        assertNotNull(response.getBody());
        var body = response.getBody();
        assertNotNull(body);
        assertTrue(body.containsKey("error"));
        assertEquals("File too large", body.get("error"));
    }

    @Test
    @DisplayName("Get system info should return system information")
    void testGetSystemInfo_Success() throws Exception {
        mockMvc.perform(get("/api/v1/files/system-info"))
            .andExpect(status().isOk())
            .andExpect(jsonPath("$.memory").exists())
            .andExpect(jsonPath("$.memory.total").exists())
            .andExpect(jsonPath("$.memory.used").exists())
            .andExpect(jsonPath("$.memory.free").exists())
            .andExpect(jsonPath("$.memory.max").exists())
            .andExpect(jsonPath("$.memory.usagePercent").exists())
            .andExpect(jsonPath("$.tempFiles").exists())
            .andExpect(jsonPath("$.tempFiles.count").exists())
            .andExpect(jsonPath("$.tempFiles.totalSize").exists())
            .andExpect(jsonPath("$.tempFiles.directory").exists())
            .andExpect(jsonPath("$.vulnerability").exists());
    }
}

